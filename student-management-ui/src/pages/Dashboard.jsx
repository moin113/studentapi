import { useEffect, useState } from 'react';
import api from '../services/api';
import { useAuth } from '../context/AuthContext';
import { Users, LogOut, Plus, Trash2, Edit } from 'lucide-react';

const Dashboard = () => {
  const [students, setStudents] = useState([]);
  const [loading, setLoading] = useState(true);
  const { logout, user } = useAuth();

  useEffect(() => {
    fetchStudents();
  }, []);

  const fetchStudents = async () => {
    try {
      const response = await api.get('/Students');
      setStudents(response.data.data || []);
    } catch (err) {
      console.error('Failed to fetch students', err);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id) => {
    if (window.confirm('Are you sure you want to delete this student?')) {
      try {
        await api.delete(`/Students/${id}`);
        fetchStudents();
      } catch (err) {
        alert('Failed to delete student. You might need Admin privileges.');
      }
    }
  };

  return (
    <div style={{ padding: '40px', maxWidth: '1200px', margin: '0 auto' }}>
      <header style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '40px' }} className="fade-in">
        <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
          <div className="btn-primary" style={{ padding: '10px' }}><Users size={24} /></div>
          <h1>Student Dashboard</h1>
        </div>
        <div style={{ display: 'flex', alignItems: 'center', gap: '20px' }}>
          <div style={{ textAlign: 'right' }}>
            <p style={{ fontWeight: 'bold' }}>{user?.fullName}</p>
            <p style={{ color: 'var(--text-muted)', fontSize: '0.8rem' }}>{user?.role}</p>
          </div>
          <button onClick={logout} className="glass-card" style={{ padding: '10px', color: 'var(--danger)', cursor: 'pointer', border: 'none' }}>
            <LogOut size={20} />
          </button>
        </div>
      </header>

      <main className="glass-card fade-in" style={{ padding: '24px' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '24px' }}>
          <h3>Student Directory</h3>
          <button className="btn-primary" style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
            <Plus size={18} /> Add Student
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
                      <button style={{ background: 'none', border: 'none', color: 'var(--text-muted)', cursor: 'pointer' }}><Edit size={18} /></button>
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
    </div>
  );
};

export default Dashboard;
