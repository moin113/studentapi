import { useEffect, useState } from 'react';
import api from '../services/api';
import { useAuth } from '../context/AuthContext';
import { Users, LogOut, Plus, Trash2, Edit, X } from 'lucide-react';

const Dashboard = () => {
  const [students, setStudents] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingStudent, setEditingStudent] = useState(null);
  const [formData, setFormData] = useState({ name: '', email: '', age: '', course: '' });
  const { logout, user } = useAuth();

  useEffect(() => {
    fetchStudents();
  }, []);

  const fetchStudents = async () => {
    try {
      const response = await api.get('/Students');
      setStudents(response.data || []);
    } catch (err) {
      console.error('Failed to fetch students', err);
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      if (editingStudent) {
        await api.put(`/Students/${editingStudent.id}`, formData);
      } else {
        await api.post('/Students', formData);
      }
      setShowModal(false);
      setEditingStudent(null);
      setFormData({ name: '', email: '', age: '', course: '' });
      fetchStudents();
    } catch (err) {
      alert(err.response?.data?.message || 'Operation failed');
    }
  };

  const handleEdit = (student) => {
    setEditingStudent(student);
    setFormData({ name: student.name, email: student.email, age: student.age, course: student.course });
    setShowModal(true);
  };

  const handleDelete = async (id) => {
    if (window.confirm('Are you sure you want to delete this student?')) {
      try {
        await api.delete(`/Students/${id}`);
        fetchStudents();
      } catch (err) {
        alert('Failed to delete student.');
      }
    }
  };

  return (
    <div style={{ padding: '40px', maxWidth: '1200px', margin: '0 auto' }}>
      <header style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '40px' }} className="fade-in">
        <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
          <div className="btn-primary" style={{ padding: '10px' }}><Users size={24} /></div>
          <h1>Admin Portal</h1>
        </div>
        <div style={{ display: 'flex', alignItems: 'center', gap: '20px' }}>
          <div style={{ textAlign: 'right' }}>
            <p style={{ fontWeight: 'bold' }}>{user?.fullName}</p>
            <span style={{ background: 'var(--success)', color: 'white', padding: '2px 8px', borderRadius: '4px', fontSize: '0.7rem' }}>ADMIN</span>
          </div>
          <button onClick={logout} className="glass-card" style={{ padding: '10px', color: 'var(--danger)', cursor: 'pointer', border: 'none' }}>
            <LogOut size={20} />
          </button>
        </div>
      </header>

      <main className="glass-card fade-in" style={{ padding: '24px' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '24px' }}>
          <h3>Manage Students</h3>
          <button onClick={() => { setEditingStudent(null); setFormData({ name: '', email: '', age: '', course: '' }); setShowModal(true); }} className="btn-primary" style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
            <Plus size={18} /> Add New Student
          </button>
        </div>

        {loading ? (
          <p>Loading students...</p>
        ) : (
          <table style={{ width: '100%', borderCollapse: 'collapse', color: 'var(--text-main)' }}>
            <thead>
              <tr style={{ borderBottom: '1px solid rgba(255,255,255,0.1)', textAlign: 'left' }}>
                <th style={{ padding: '12px' }}>Name</th>
                <th style={{ padding: '12px' }}>Email</th>
                <th style={{ padding: '12px' }}>Course</th>
                <th style={{ padding: '12px' }}>Age</th>
                <th style={{ padding: '12px' }}>Actions</th>
              </tr>
            </thead>
            <tbody>
              {students.map((student) => (
                <tr key={student.id} style={{ borderBottom: '1px solid rgba(255,255,255,0.05)' }}>
                  <td style={{ padding: '12px' }}>{student.name}</td>
                  <td style={{ padding: '12px', color: 'var(--text-muted)' }}>{student.email}</td>
                  <td style={{ padding: '12px' }}>
                    <span style={{ background: 'rgba(99, 102, 241, 0.2)', color: 'var(--primary)', padding: '4px 12px', borderRadius: '20px', fontSize: '0.8rem' }}>
                      {student.course}
                    </span>
                  </td>
                  <td style={{ padding: '12px' }}>{student.age}</td>
                  <td style={{ padding: '12px' }}>
                    <div style={{ display: 'flex', gap: '12px' }}>
                      <button onClick={() => handleEdit(student)} style={{ background: 'none', border: 'none', color: 'var(--text-muted)', cursor: 'pointer' }}><Edit size={18} /></button>
                      <button 
                        onClick={() => handleDelete(student.id)}
                        style={{ background: 'none', border: 'none', color: 'var(--danger)', cursor: 'pointer' }}
                      >
                        <Trash2 size={18} />
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </main>

      {showModal && (
        <div style={{ position: 'fixed', top: 0, left: 0, width: '100%', height: '100%', background: 'rgba(0,0,0,0.8)', display: 'flex', justifyContent: 'center', alignItems: 'center', zIndex: 1000 }}>
          <div className="glass-card fade-in" style={{ width: '500px', padding: '32px', position: 'relative' }}>
            <button onClick={() => setShowModal(false)} style={{ position: 'absolute', top: 16, right: 16, background: 'none', border: 'none', color: 'white', cursor: 'pointer' }}>
              <X size={24} />
            </button>
            <h2 style={{ marginBottom: '24px' }}>{editingStudent ? 'Edit Student' : 'Add New Student'}</h2>
            <form onSubmit={handleSubmit}>
              <label className="text-muted">Name</label>
              <input type="text" className="input-field" value={formData.name} onChange={(e) => setFormData({...formData, name: e.target.value})} required />
              
              <label className="text-muted">Email</label>
              <input type="email" className="input-field" value={formData.email} onChange={(e) => setFormData({...formData, email: e.target.value})} required />
              
              <div style={{ display: 'flex', gap: '20px' }}>
                <div style={{ flex: 1 }}>
                  <label className="text-muted">Age</label>
                  <input type="number" className="input-field" value={formData.age} onChange={(e) => setFormData({...formData, age: e.target.value})} required />
                </div>
                <div style={{ flex: 2 }}>
                  <label className="text-muted">Course</label>
                  <input type="text" className="input-field" value={formData.course} onChange={(e) => setFormData({...formData, course: e.target.value})} required />
                </div>
              </div>
              
              <button type="submit" className="btn-primary" style={{ width: '100%', marginTop: '20px' }}>
                {editingStudent ? 'Update Details' : 'Save Student'}
              </button>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};

export default Dashboard;
